<template>
  <div>
    <div class="page-header">
      <h1>Pedidos</h1>
    </div>

    <div v-if="loading" class="status">Carregando...</div>

    <div v-else-if="orders.length === 0" class="status">
      Nenhum pedido cadastrado.
    </div>

    <div v-else class="table-wrapper">
      <table>
        <thead>
          <tr>
            <th>ID</th>
            <th>Cliente</th>
            <th class="text-right">Valor</th>
            <th>Data</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="order in orders" :key="order.id">
            <td>#{{ order.id }}</td>
            <td>{{ order.cliente }}</td>
            <td class="text-right">{{ formatCurrency(order.valor) }}</td>
            <td>{{ formatDate(order.dataPedido) }}</td>
            <td><router-link :to="`/orders/${order.id}`">Detalhes</router-link></td>
          </tr>
        </tbody>
      </table>
    </div>

    <p v-if="erro" class="error-message">{{ erro }}</p>
  </div>
</template>

<script setup>
  import { ref, onMounted, watch } from 'vue'
  import { useRoute } from 'vue-router'

  const API_BASE = 'http://localhost:5000'

  const route = useRoute()
  const orders = ref([])
  const loading = ref(true)
  const erro = ref(null)

  const carregarPedidos = async () => {
    loading.value = true
    try {
      const response = await fetch(`${API_BASE}/orders`)
      if (!response.ok) throw new Error('Erro ao carregar pedidos')
      orders.value = await response.json()
    } catch (e) {
      erro.value = e.message
    } finally {
      loading.value = false
    }
  }

  onMounted(carregarPedidos)

  watch(() => route.query.refresh, (newVal) => {
    if (newVal) carregarPedidos()
  })

  const formatCurrency = (value) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value)
  }

  const formatDate = (dateStr) => {
    return new Date(dateStr).toLocaleString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    })
  }
</script>
