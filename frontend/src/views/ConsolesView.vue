<template>
  <div>
    <div class="page-header">
      <h1>Consoles</h1>
      <button class="btn btn-primary" @click="openCreateModal">+ Add Console</button>
    </div>

    <Notification
      v-if="notification.message"
      :message="notification.message"
      :type="notification.type"
      @close="notification.message = ''"
    />

    <div class="search-bar">
      <input
        v-model="searchQuery"
        placeholder="Search by console or manufacturer name..."
        @input="debouncedSearch"
      />
    </div>

    <div class="card">
      <table>
        <thead>
          <tr>
            <th @click="sortBy('name')" class="sortable">
              Name {{ getSortIcon('name') }}
            </th>
            <th @click="sortBy('manufacturerName')" class="sortable">
              Manufacturer {{ getSortIcon('manufacturerName') }}
            </th>
            <th @click="sortBy('releaseYear')" class="sortable">
              Year {{ getSortIcon('releaseYear') }}
            </th>
            <th @click="sortBy('generation')" class="sortable">
              Gen {{ getSortIcon('generation') }}
            </th>
            <th @click="sortBy('unitsSoldMillions')" class="sortable">
              Units Sold (M) {{ getSortIcon('unitsSoldMillions') }}
            </th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="loading">
            <td colspan="7" class="text-center">Loading...</td>
          </tr>
          <tr v-else-if="sorted.length === 0">
            <td colspan="7" class="text-center">No consoles found.</td>
          </tr>
          <tr v-for="c in sorted" :key="c.id">
            <td>{{ c.name }}</td>
            <td>{{ c.manufacturerName }}</td>
            <td>{{ c.releaseYear }}</td>
            <td>{{ c.generation }}</td>
            <td>{{ c.unitsSoldMillions ?? '—' }}</td>
            <td>
              <span :class="c.isDiscontinued ? 'badge-discontinued' : 'badge-active'">
                {{ c.isDiscontinued ? 'Discontinued' : 'Active' }}
              </span>
            </td>
            <td>
              <button class="btn btn-sm" @click="openEditModal(c)">Edit</button>
              <button class="btn btn-sm btn-danger" @click="confirmDelete(c)">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Create / Edit Modal -->
    <div v-if="showModal" class="modal-overlay" @click.self="closeModal">
      <div class="modal">
        <h2>{{ editing ? 'Edit Console' : 'Add Console' }}</h2>
        <form @submit.prevent="save">
          <div class="form-group">
            <label>Manufacturer *</label>
            <select v-model="form.manufacturerId" required>
              <option value="">Select manufacturer...</option>
              <option v-for="m in manufacturers" :key="m.id" :value="m.id">
                {{ m.name }}
              </option>
            </select>
          </div>
          <div class="form-group">
            <label>Console Name *</label>
            <input v-model="form.name" required maxlength="100" />
          </div>
          <div class="form-group">
            <label>Release Year *</label>
            <input
              v-model.number="form.releaseYear"
              type="number"
              required
              min="1800"
              :max="currentYear"
            />
          </div>
          <div class="form-group">
            <label>Generation *</label>
            <input
              v-model.number="form.generation"
              type="number"
              required
              min="1"
              max="20"
            />
          </div>
          <div class="form-group">
            <label>Units Sold (millions)</label>
            <input
              v-model.number="form.unitsSoldMillions"
              type="number"
              step="0.01"
              min="0"
            />
          </div>
          <div class="form-group checkbox">
            <label>
              <input v-model="form.isDiscontinued" type="checkbox" />
              Discontinued
            </label>
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
import { consoleService }      from '../services/consoleService'
import { manufacturerService } from '../services/manufacturerService'
import Notification            from '../components/Notification.vue'

const consoles      = ref([])
const manufacturers = ref([])
const loading       = ref(false)
const saving        = ref(false)
const searchQuery   = ref('')
const showModal     = ref(false)
const editing       = ref(null)
const deleteTarget  = ref(null)
const formError     = ref('')
const currentYear   = new Date().getFullYear()
const notification  = ref({ message: '', type: 'success' })
const sortKey       = ref('name')
const sortOrder     = ref('asc')

const emptyForm = () => ({
  manufacturerId:    '',
  name:              '',
  releaseYear:       currentYear,
  generation:        1,
  unitsSoldMillions: null,
  isDiscontinued:    true
})

const form = ref(emptyForm())

// ── Sorting ──────────────────────────────────────────────────
const sorted = computed(() => {
  return [...consoles.value].sort((a, b) => {
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
    sortKey.value   = key
    sortOrder.value = 'asc'
  }
}

function getSortIcon(key) {
  if (sortKey.value !== key) return '⇅'
  return sortOrder.value === 'asc' ? '↑' : '↓'
}

// ── Debounced search — waits 300ms after typing stops ────────
let searchTimer = null
function debouncedSearch() {
  clearTimeout(searchTimer)
  searchTimer = setTimeout(fetchConsoles, 300)
}

// ── Data fetching ─────────────────────────────────────────────
async function fetchConsoles() {
  loading.value = true
  try {
    const response  = await consoleService.getAll(searchQuery.value)
    consoles.value  = response.data
  } catch (err) {
    showNotification(err.message, 'error')
  } finally {
    loading.value = false
  }
}

async function fetchManufacturers() {
  try {
    const response      = await manufacturerService.getAll()
    manufacturers.value = response.data
  } catch (err) {
    showNotification(err.message, 'error')
  }
}

// ── Modal controls ────────────────────────────────────────────
function openCreateModal() {
  editing.value   = null
  form.value      = emptyForm()
  formError.value = ''
  showModal.value = true
}

function openEditModal(c) {
  editing.value   = c
  form.value      = { ...c }
  formError.value = ''
  showModal.value = true
}

function closeModal() {
  showModal.value = false
}

function confirmDelete(c) {
  deleteTarget.value = c
}

// ── Save (create or update) ───────────────────────────────────
async function save() {
  formError.value = ''
  saving.value    = true
  try {
    if (editing.value) {
      await consoleService.update(editing.value.id, form.value)
      showNotification('Console updated successfully!')
    } else {
      await consoleService.create(form.value)
      showNotification('Console created successfully!')
    }
    closeModal()
    await fetchConsoles()
  } catch (err) {
    formError.value = err.message
  } finally {
    saving.value = false
  }
}

// ── Delete ────────────────────────────────────────────────────
async function executeDelete() {
  saving.value = true
  try {
    await consoleService.delete(deleteTarget.value.id)
    showNotification('Console deleted.')
    deleteTarget.value = null
    await fetchConsoles()
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

onMounted(async () => {
  await Promise.all([fetchConsoles(), fetchManufacturers()])
})
</script>

<style scoped>
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}
.search-bar {
  margin-bottom: 1rem;
}
.search-bar input {
  width: 100%;
  padding: 0.6rem 0.8rem;
  border: 1px solid #ddd;
  border-radius: 6px;
  font-size: 1rem;
}
.card {
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.08);
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
  font-size: 0.9rem;
}
th.sortable {
  cursor: pointer;
  user-select: none;
}
th.sortable:hover { background: #efefef; }
.text-center { text-align: center; color: #888; padding: 2rem; }
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
.btn-sm      { font-size: 0.8rem;  padding: 0.25rem 0.6rem; }
.btn:disabled { opacity: 0.6; cursor: not-allowed; }
.badge-discontinued {
  background: #e0e0e0;
  color: #555;
  padding: 2px 8px;
  border-radius: 12px;
  font-size: 0.8rem;
}
.badge-active {
  background: #d4edda;
  color: #155724;
  padding: 2px 8px;
  border-radius: 12px;
  font-size: 0.8rem;
}
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
  max-width: 500px;
  max-height: 90vh;
  overflow-y: auto;
}
.modal h2 { margin-bottom: 1.5rem; }
.form-group { margin-bottom: 1rem; }
.form-group label {
  display: block;
  margin-bottom: 0.25rem;
  font-weight: 500;
  font-size: 0.9rem;
}
.form-group input,
.form-group select {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
}
.form-group.checkbox label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-weight: normal;
}
.form-group.checkbox input {
  width: auto;
}
.form-error {
  color: #dc3545;
  font-size: 0.9rem;
  margin-top: 0.5rem;
}
.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.5rem;
  margin-top: 1.5rem;
}
</style>