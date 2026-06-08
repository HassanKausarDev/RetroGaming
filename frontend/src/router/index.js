import { createRouter, createWebHistory } from 'vue-router'
import ConsolesView from '../views/ConsolesView.vue'
import ManufacturersView from '../views/ManufacturersView.vue'
import MapView from '../views/MapView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: '/',               redirect: '/consoles' },
    { path: '/consoles',       component: ConsolesView },
    { path: '/manufacturers',  component: ManufacturersView },
    { path: '/map',            component: MapView }
  ]
})

export default router