<template>
  <div>
    <h1>Novo Pedido</h1>

    <div class="form-card">
      <div class="form-group">
        <label for="cliente">Cliente</label>
        <input
          id="cliente"
          v-model="cliente"
          type="text"
          class="form-control"
          placeholder="Nome do cliente"
        />
      </div>

      <div class="form-group">
        <label for="valor">Valor</label>
        <input
          id="valor"
          v-model="valorTexto"
          type="text"
          class="form-control"
          placeholder="Ex: 10.25"
        />
      </div>

      <div class="form-actions">
        <button class="btn-primary" :disabled="salvando" @click="salvar">
          {{ salvando ? 'Enviando...' : 'Cadastrar' }}
        </button>
        <router-link to="/" class="btn-link">Cancelar</router-link>
      </div>

      <p v-if="erro" class="error-message">{{ erro }}</p>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const API_BASE = 'http://localhost:5000'

const router = useRouter()
const cliente = ref('')
const valorTexto = ref('')
const salvando = ref(false)
const erro = ref(null)

const salvar = async () => {
  erro.value = null

  if (!cliente.value.trim()) {
    erro.value = 'Informe o cliente.'
    return
  }

  const valor = parseFloat(valorTexto.value.replace(',', '.'))
  if (isNaN(valor) || valor <= 0) {
    erro.value = 'Informe um valor válido maior que zero.'
    return
  }

  salvando.value = true

  try {
    const response = await fetch(`${API_BASE}/orders`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ cliente: cliente.value, valor })
    })

    if (!response.ok) throw new Error('Erro ao cadastrar pedido')

    router.push('/')
  } catch (e) {
    erro.value = `Erro ao cadastrar: ${e.message}`
    salvando.value = false
  }
}
</script>
