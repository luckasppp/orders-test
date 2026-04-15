<template>
  <div>
    <nav class="navbar">
      <div class="navbar-inner">
        <router-link to="/" class="brand">OrdersTest</router-link>
        <div class="nav-links">
          <router-link to="/" class="nav-link">Pedidos</router-link>
          <router-link to="/orders/new" class="nav-link nav-link-accent">+ Novo Pedido</router-link>
        </div>
      </div>
    </nav>
    <main class="container">
      <router-view />
    </main>
  </div>
</template>

<style>
  :root {
    --color-primary: #2563eb;
    --color-primary-hover: #1d4ed8;
    --color-border: #e5e7eb;
    --color-text: #111827;
    --color-text-muted: #6b7280;
    --color-bg: #f9fafb;
    --color-card: #ffffff;
    --color-danger: #dc2626;
  }

  * {
    box-sizing: border-box;
  }

  body {
    margin: 0;
    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
    background: var(--color-bg);
    color: var(--color-text);
    font-size: 15px;
    line-height: 1.5;
  }

  h1 {
    font-size: 1.75rem;
    margin: 0 0 1.5rem 0;
    color: var(--color-text);
  }

  a {
    text-decoration: none;
  }

  .navbar {
    background: #1f2937;
    height: 64px;
    position: sticky;
    top: 0;
    z-index: 100;
  }

  .navbar-inner {
    max-width: 1000px;
    margin: 0 auto;
    padding: 0 1.5rem;
    height: 100%;
    display: flex;
    align-items: center;
    justify-content: space-between;
  }

  .brand {
    color: white;
    font-weight: 700;
    font-size: 1.1rem;
    letter-spacing: -0.01em;
  }

  .brand:hover {
    color: #e5e7eb;
  }

  .nav-links {
    display: flex;
    align-items: center;
    gap: 0.75rem;
  }

  .nav-link {
    color: #d1d5db;
    padding: 0.4rem 0.75rem;
    border-radius: 6px;
    font-size: 0.9rem;
    font-weight: 500;
    transition: all 0.15s;
  }

  .nav-link:hover {
    color: white;
    background: rgba(255,255,255,0.1);
  }

  .nav-link-accent {
    background: var(--color-primary);
    color: white !important;
    font-size: 0.85rem;
  }

  .nav-link-accent:hover {
    background: var(--color-primary-hover);
  }

  .container {
    max-width: 1000px;
    margin: 0 auto;
    padding: 2.5rem 1.5rem;
  }

  .btn-primary {
    display: inline-block;
    background: var(--color-primary);
    color: white;
    padding: 0.6rem 1.2rem;
    border-radius: 6px;
    font-weight: 500;
    font-size: 0.9rem;
    border: none;
    cursor: pointer;
    transition: background 0.15s;
    font-family: inherit;
  }

  .btn-primary:hover:not(:disabled) {
    background: var(--color-primary-hover);
    color: white;
  }

  .btn-primary:disabled {
    opacity: 0.6;
    cursor: not-allowed;
  }

  .btn-link {
    color: var(--color-text-muted);
    font-size: 0.9rem;
  }

  .btn-link:hover {
    color: var(--color-text);
  }

  .status {
    color: var(--color-text-muted);
    padding: 2rem;
    text-align: center;
    background: var(--color-card);
    border-radius: 8px;
    border: 1px dashed var(--color-border);
  }

  .table-wrapper {
    background: var(--color-card);
    border-radius: 8px;
    overflow: hidden;
    box-shadow: 0 1px 3px rgba(0,0,0,0.05);
  }

  table {
    width: 100%;
    border-collapse: collapse;
  }

  th {
    text-align: left;
    padding: 0.75rem 1rem;
    font-size: 0.85rem;
    font-weight: 600;
    color: var(--color-text-muted);
    text-transform: uppercase;
    letter-spacing: 0.02em;
    background: var(--color-bg);
    border-bottom: 1px solid var(--color-border);
  }

  td {
    padding: 0.75rem 1rem;
    border-bottom: 1px solid var(--color-border);
  }

  tbody tr:last-child td {
    border-bottom: none;
  }

  tbody tr:hover {
    background: var(--color-bg);
  }

  .text-right {
    text-align: right;
  }

  .form-card {
    background: var(--color-card);
    padding: 2rem;
    border-radius: 8px;
    max-width: 500px;
    box-shadow: 0 1px 3px rgba(0,0,0,0.05);
  }

  .form-group {
    margin-bottom: 1.25rem;
  }

  label {
    display: block;
    margin-bottom: 0.4rem;
    font-weight: 500;
    font-size: 0.9rem;
  }

  .form-control {
    width: 100%;
    padding: 0.6rem 0.75rem;
    font-size: 1rem;
    border: 1px solid var(--color-border);
    border-radius: 6px;
    font-family: inherit;
    transition: border-color 0.15s;
    background: white;
  }

  .form-control:focus {
    outline: none;
    border-color: var(--color-primary);
    box-shadow: 0 0 0 3px rgba(37,99,235,0.1);
  }

  .form-actions {
    display: flex;
    align-items: center;
    gap: 1rem;
    margin-top: 1.5rem;
  }

  .error-message {
    color: var(--color-danger);
    margin-top: 1rem;
    padding: 0.75rem;
    background: #fef2f2;
    border-radius: 6px;
    font-size: 0.9rem;
  }

  .page-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 1.5rem;
  }

  .page-header h1 {
    margin: 0;
  }

  .d-flex {
    display: flex;
  }

  .justify-end {
    justify-content: space-around;
  }
</style>
