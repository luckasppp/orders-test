import { createApp } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import App from './App.vue'
import HomeView from './views/HomeView.vue'
import NewOrderView from './views/NewOrderView.vue'
import OrderDetailView from './views/OrderDetailView.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', component: HomeView },
    { path: '/orders/new', component: NewOrderView },
    { path: '/orders/:id', component: OrderDetailView }
  ]
})

const app = createApp(App)
app.use(router)
app.mount('#app')
