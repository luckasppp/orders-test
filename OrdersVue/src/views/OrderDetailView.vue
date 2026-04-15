<template>
  <div>
    <h1>Pedido #{{ order?.id }}</h1>
    <p v-if="loading">Carregando...</p>
    <div v-else-if="order">
      <div class="form-card">
        <div class="detail-row">
          <span class="label">Cliente</span>
          <span>{{ order.cliente }}</span>
        </div>
        <div class="detail-row">
          <span class="label">Valor</span>
          <span>{{ formatCurrency(order.valor) }}</span>
        </div>
        <div class="detail-row">
          <span class="label">Data</span>
          <span>{{ formatDate(order.dataPedido) }}</span>
        </div>
        <div class="form-actions d-flex justify-end" style="margin-top: 1.5rem;">
          <router-link to="/" class="btn-primary">Voltar</router-link>
        </div>
      </div>
    </div>
    <p v-else class="error-message">Pedido não encontrado.</p>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'

const API_BASE = 'http://localhost:5000'

const route = useRoute()
const order = ref(null)
const loading = ref(true)

onMounted(async () => {
  try {
    const response = await fetch(`${API_BASE}/orders/${route.params.id}`)
    if (!response.ok) throw new Error('Pedido não encontrado')
    order.value = await response.json()
  } catch {
    order.value = null
  } finally {
    loading.value = false
  }
})

const formatCurrency = (value) => {
  return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value)
}

const formatDate = (dateStr) => {
  return new Date(dateStr).toLocaleString('pt-BR', {
    day: '2-digit', month: '2-digit', year: 'numeric',
    hour: '2-digit', minute: '2-digit'
  })
}
</script>

<style scoped>
.detail-row {
  display: flex;
  justify-content: space-between;
  padding: 0.75rem 0;
  border-bottom: 1px solid var(--color-border);
}

.detail-row:last-child {
  border-bottom: none;
}

.label {
  font-weight: 500;
  color: var(--color-text-muted);
}
</style>