<template>
  <div>
    <div class="page-header">
      <h1>Manufacturers</h1>
      <button class="btn btn-primary" @click="openCreateModal">+ Add Manufacturer</button>
    </div>

    <Notification
      v-if="notification.message"
      :message="notification.message"
      :type="notification.type"
      @close="notification.message = ''"
    />

    <div class="card">
      <table>
        <thead>
          <tr>
            <th @click="sortBy('name')" class="sortable">Name {{ getSortIcon('name') }}</th>
            <th @click="sortBy('country')" class="sortable">Country {{ getSortIcon('country') }}</th>
            <th>City</th>
            <th @click="sortBy('foundedYear')" class="sortable">Founded {{ getSortIcon('foundedYear') }}</th>
            <th>Consoles</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="loading">
            <td colspan="6">Loading...</td>
          </tr>
          <tr v-else-if="sorted.length === 0">
            <td colspan="6">No manufacturers found.</td>
          </tr>
          <tr v-for="m in sorted" :key="m.id">
            <td>{{ m.name }}</td>
            <td>{{ m.country }}</td>
            <td>{{ m.city }}</td>
            <td>{{ m.foundedYear }}</td>
            <td>{{ m.consoleCount }}</td>
            <td>
              <button class="btn btn-sm" @click="openEditModal(m)">Edit</button>
              <button class="btn btn-sm btn-danger" @click="confirmDelete(m)">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Create / Edit Modal -->
    <div v-if="showModal" class="modal-overlay" @click.self="closeModal">
      <div class="modal">
        <h2>{{ editing ? 'Edit Manufacturer' : 'Add Manufacturer' }}</h2>
        <form @submit.prevent="save">
          <div class="form-group">
            <label>Name *</label>
            <input v-model="form.name" required maxlength="100" />
          </div>
          <div class="form-group">
            <label>Country *</label>
            <input v-model="form.country" required maxlength="100" />
          </div>
          <div class="form-group">
            <label>City *</label>
            <input v-model="form.city" required maxlength="100" />
          </div>
          <div class="form-group">
            <label>Founded Year *</label>
            <input v-model.number="form.foundedYear" type="number" required min="1800" :max="currentYear" />
          </div>
          <div class="form-group">
            <label>Latitude <span class="hint">(e.g. 35.011635)</span></label>
            <input v-model.number="form.latitude" type="number" step="0.000001" min="-90" max="90" />
          </div>
          <div class="form-group">
            <label>Longitude <span class="hint">(e.g. 135.768150)</span></label>
            <input v-model.number="form.longitude" type="number" step="0.000001" min="-180" max="180" />
          </div>
          <p v-if="formError" class="form-error">{{ formError }}</p>
          <div class="modal-actions">
            <button type="button" class="btn" @click="closeModal">Cancel</button>
            <button type="submit" class="btn btn-primary" :disabled="saving">
              {{ saving ? 'Saving...' : 'Save' }}
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Delete Confirmation Modal -->
    <div v-if="deleteTarget" class="modal-overlay" @click.self="deleteTarget = null">
      <div class="modal">
        <h2>Confirm Delete</h2>
        <p>Are you sure you want to delete <strong>{{ deleteTarget.name }}</strong>?</p>
        <p class="hint" style="margin-top:0.5rem">This will fail if they still have consoles linked.</p>
        <div class="modal-actions">
          <button class="btn" @click="deleteTarget = null">Cancel</button>
          <button class="btn btn-danger" @click="executeDelete" :disabled="saving">
            {{ saving ? 'Deleting...' : 'Delete' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { manufacturerService } from '../services/manufacturerService'
import Notification from '../components/Notification.vue'

const manufacturers = ref([])
const loading       = ref(false)
const saving        = ref(false)
const showModal     = ref(false)
const editing       = ref(null)
const deleteTarget  = ref(null)
const formError     = ref('')
const currentYear   = new Date().getFullYear()
const notification  = ref({ message: '', type: 'success' })
const sortKey       = ref('name')
const sortOrder     = ref('asc')

const emptyForm = () => ({
  name:        '',
  country:     '',
  city:        '',
  foundedYear: currentYear,
  latitude:    null,
  longitude:   null
})

const form = ref(emptyForm())

const sorted = computed(() => {
  return [...manufacturers.value].sort((a, b) => {
    let valA = a[sortKey.value]
    let valB = b[sortKey.value]
    if (valA == null) return 1
    if (valB == null) return -1
    if (typeof valA === 'string') valA = valA.toLowerCase()
    if (typeof valB === 'string') valB = valB.toLowerCase()
    return sortOrder.value === 'asc'
      ? (valA > valB ? 1 : -1)
      : (valA < valB ? 1 : -1)
  })
})

function sortBy(key) {
  if (sortKey.value === key) {
    sortOrder.value = sortOrder.value === 'asc' ? 'desc' : 'asc'
  } else {
    sortKey.value  = key
    sortOrder.value = 'asc'
  }
}

function getSortIcon(key) {
  if (sortKey.value !== key) return '⇅'
  return sortOrder.value === 'asc' ? '↑' : '↓'
}

async function fetchManufacturers() {
  loading.value = true
  try {
    const response      = await manufacturerService.getAll()
    manufacturers.value = response.data
  } catch (err) {
    showNotification(err.message, 'error')
  } finally {
    loading.value = false
  }
}

function openCreateModal() {
  editing.value   = null
  form.value      = emptyForm()
  formError.value = ''
  showModal.value = true
}

function openEditModal(m) {
  editing.value   = m
  form.value      = { ...m }
  formError.value = ''
  showModal.value = true
}

function closeModal() {
  showModal.value = false
}

function confirmDelete(m) {
  deleteTarget.value = m
}

async function save() {
  formError.value = ''
  saving.value    = true
  try {
    if (editing.value) {
      await manufacturerService.update(editing.value.id, form.value)
      showNotification('Manufacturer updated successfully!')
    } else {
      await manufacturerService.create(form.value)
      showNotification('Manufacturer created successfully!')
    }
    closeModal()
    await fetchManufacturers()
  } catch (err) {
    formError.value = err.message
  } finally {
    saving.value = false
  }
}

async function executeDelete() {
  saving.value = true
  try {
    await manufacturerService.delete(deleteTarget.value.id)
    showNotification('Manufacturer deleted.')
    deleteTarget.value = null
    await fetchManufacturers()
  } catch (err) {
    showNotification(err.message, 'error')
    deleteTarget.value = null
  } finally {
    saving.value = false
  }
}

function showNotification(message, type = 'success') {
  notification.value = { message, type }
  setTimeout(() => { notification.value.message = '' }, 4000)
}

onMounted(fetchManufacturers)
</script>

<style scoped>
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}
.card {
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
  overflow: hidden;
}
table {
  width: 100%;
  border-collapse: collapse;
}
th, td {
  padding: 0.75rem 1rem;
  text-align: left;
  border-bottom: 1px solid #eee;
}
th {
  background: #f8f8f8;
  font-weight: 600;
}
th.sortable {
  cursor: pointer;
  user-select: none;
}
th.sortable:hover { background: #efefef; }
.btn {
  padding: 0.4rem 0.9rem;
  border: 1px solid #ccc;
  border-radius: 4px;
  cursor: pointer;
  background: white;
  margin-right: 0.25rem;
  font-size: 0.9rem;
}
.btn-primary { background: #1a1a2e; color: white; border-color: #1a1a2e; }
.btn-danger  { background: #dc3545; color: white; border-color: #dc3545; }
.btn-sm      { font-size: 0.8rem; padding: 0.25rem 0.6rem; }
.btn:disabled { opacity: 0.6; cursor: not-allowed; }
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}
.modal {
  background: white;
  border-radius: 8px;
  padding: 2rem;
  width: 100%;
  max-width: 480px;
}
.modal h2 { margin-bottom: 1.5rem; }
.form-group { margin-bottom: 1rem; }
.form-group label {
  display: block;
  margin-bottom: 0.25rem;
  font-weight: 500;
  font-size: 0.9rem;
}
.form-group input {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
}
.form-error { color: #dc3545; margin-top: 0.5rem; font-size: 0.9rem; }
.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.5rem;
  margin-top: 1.5rem;
}
.hint { color: #888; font-size: 0.8rem; }
</style>