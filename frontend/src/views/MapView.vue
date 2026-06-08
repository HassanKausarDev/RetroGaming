<template>
  <div>
    <div class="page-header">
      <h1>Manufacturer Locations</h1>
      <p class="subtitle">Click a marker to see manufacturer details.</p>
    </div>

    <Notification
      v-if="notification.message"
      :message="notification.message"
      :type="notification.type"
      @close="notification.message = ''"
    />

    <div v-if="loading" class="loading">Loading map data...</div>

    <div id="map" ref="mapContainer"></div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'
import { manufacturerService } from '../services/manufacturerService'
import Notification from '../components/Notification.vue'

// Fix Leaflet marker icons broken by Vite's bundling
import markerIcon2x from 'leaflet/dist/images/marker-icon-2x.png'
import markerIcon   from 'leaflet/dist/images/marker-icon.png'
import markerShadow from 'leaflet/dist/images/marker-shadow.png'

delete L.Icon.Default.prototype._getIconUrl
L.Icon.Default.mergeOptions({
  iconUrl:       markerIcon,
  iconRetinaUrl: markerIcon2x,
  shadowUrl:     markerShadow
})

const mapContainer = ref(null)
const loading      = ref(true)
const notification = ref({ message: '', type: 'success' })
let map            = null

onMounted(async () => {
  // Initialise Leaflet map centred on the world
  map = L.map(mapContainer.value).setView([30, 10], 2)

  // OpenStreetMap tiles — completely free, no API key needed
  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '© <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
    maxZoom: 18
  }).addTo(map)

  try {
    const response     = await manufacturerService.getMapData()
    const manufacturers = response.data

    if (manufacturers.length === 0) {
      notification.value = {
        message: 'No manufacturers with map coordinates found.',
        type: 'error'
      }
      return
    }

    manufacturers.forEach(m => {
      const marker = L.marker([m.latitude, m.longitude]).addTo(map)
      marker.bindPopup(`
        <div style="min-width:160px">
          <strong style="font-size:1rem">${m.name}</strong><br/>
          <span>📍 ${m.city}, ${m.country}</span><br/>
          <span>🎮 ${m.consoleCount} console(s)</span>
        </div>
      `)
    })
  } catch (err) {
    notification.value = { message: err.message, type: 'error' }
  } finally {
    loading.value = false
  }
})

// Clean up the map instance when navigating away
onUnmounted(() => {
  if (map) {
    map.remove()
    map = null
  }
})
</script>

<style scoped>
.page-header { margin-bottom: 1rem; }
.subtitle    { color: #666; margin-top: 0.25rem; }
.loading     { padding: 1rem; color: #666; }
#map {
  height: 600px;
  border-radius: 8px;
  border: 1px solid #ddd;
  z-index: 0;
}
</style>