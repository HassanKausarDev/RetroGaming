<template>
  <div v-if="message" :class="['notification', type]">
    <span>{{ message }}</span>
    <button @click="$emit('close')">×</button>
  </div>
</template>

<script setup>
defineProps({
  message: String,
  type: {
    type: String,
    default: 'success'
  }
})
defineEmits(['close'])
</script>

<style scoped>
.notification {
  padding: 0.9rem 1rem;
  border-radius: 6px;
  margin-bottom: 1rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 0.95rem;
}
.notification.success {
  background: #d4edda;
  color: #155724;
  border: 1px solid #c3e6cb;
}
.notification.error {
  background: #f8d7da;
  color: #721c24;
  border: 1px solid #f5c6cb;
}
.notification button {
  background: none;
  border: none;
  cursor: pointer;
  font-size: 1.3rem;
  line-height: 1;
  color: inherit;
  padding: 0 0.25rem;
}
</style>