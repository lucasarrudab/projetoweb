import './style.css';

let currentView = 'auth';
let currentUser = null;
let currentTab = 'login';

// Temporary data storage - will be replaced with API calls
const products = [];

function showAuth() {
  document.querySelector('#app').innerHTML = `
    <div class="min-h-screen flex flex-col items-center justify-center bg-bakery-cream">
      <div class="mb-8">
        <img src="https://images.pexels.com/photos/1435735/pexels-photo-1435735.jpeg" 
             alt="Bakery Icon" 
             class="w-24 h-24 rounded-full object-cover border-4 border-bakery-brown shadow-lg"
        />
      </div>
      <div class="bg-white p-8 rounded-lg shadow-lg w-96">
        <h2 class="text-3xl font-bold text-center mb-6 text-bakery-brown">Nossa Padaria</h2>
        <div class="flex gap-4 mb-6">
          <button onclick="window.switchTab('login')" class="auth-tab flex-1 ${currentTab === 'login' ? 'auth-tab-active' : 'auth-tab-inactive'}">Login</button>
          <button onclick="window.switchTab('signup')" class="auth-tab flex-1 ${currentTab === 'signup' ? 'auth-tab-active' : 'auth-tab-inactive'}">Sign Up</button>
        </div>
        <form onsubmit="window.handleAuth(event)">
          <input type="email" placeholder="Email" required class="auth-input" id="email">
          <input type="password" placeholder="Password" required class="auth-input mb-2" id="password">
          <button type="button" onclick="window.handleForgotPassword()" class="text-sm text-bakery-brown hover:underline mb-4">
            Forgot Password?
            </button>
          <button type="submit" class="auth-button mt-6">${currentTab === 'login' ? 'Login' : 'Sign Up'}</button>
        </form>
      </div>
    </div>
  `;
}

function showProducts() {
  document.querySelector('#app').innerHTML = `
    <div class="min-h-screen bg-bakery-cream p-8">
      <div class="max-w-6xl mx-auto">
        <div class="flex justify-between items-center mb-8">
          <h1 class="text-4xl font-bold text-bakery-brown">Bakery Products</h1>
          <button onclick="window.showAddProductModal()" class="auth-button px-4 w-auto">Add Product</button>
        </div>
        ${products.length === 0 ? `
          <div class="flex flex-col items-center justify-center bg-white rounded-lg p-12 shadow-lg">
            <div class="text-6xl mb-4">😢</div>
            <h2 class="text-2xl font-bold text-bakery-brown mb-4">No Products Registered</h2>
            <button onclick="window.showAddProductModal()" class="auth-button px-8 py-3">Add Your First Product</button>
          </div>
        ` : `
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6" id="productsList">
            ${products.map(product => `
              <div class="product-card">
                <h3 class="text-xl font-bold mb-2">${product.name}</h3>
                <p class="text-gray-600 mb-2">${product.description}</p>
                <p class="text-bakery-brown font-bold">$${product.price}</p>
                <div class="flex gap-2 mt-4">
                  <button onclick="window.editProduct(${product.id})" class="auth-button px-4 w-auto">Edit</button>
                  <button onclick="window.deleteProduct(${product.id})" class="bg-red-500 text-white px-4 py-2 rounded-lg hover:bg-opacity-90">Delete</button>
                </div>
              </div>
            `).join('')}
          </div>
        `}
      </div>
    </div>
    <div id="productModal" class="modal hidden" onclick="if(event.target === this) window.closeProductModal()">
      <div class="modal-content">
        <div class="flex justify-between items-center mb-4">
          <h2 class="text-2xl font-bold">Add New Product</h2>
          <button onclick="window.closeProductModal()" class="text-gray-500 hover:text-gray-700">
            <span class="text-2xl">&times;</span>
          </button>
        </div>
        <form id="productForm" onsubmit="window.handleProductSubmit(event)">
          <input type="text" placeholder="Product Name" required class="auth-input" id="productName">
          <textarea placeholder="Description" required class="auth-input" id="productDescription"></textarea>
          <input type="number" step="0.01" placeholder="Price" required class="auth-input" id="productPrice">
          <div class="flex gap-4">
            <button type="submit" class="auth-button">Save Product</button>
            <button type="button" onclick="window.closeProductModal()" class="auth-button bg-gray-500">Cancel</button>
          </div>
        </form>
      </div>
    </div>
  `;
}

function handleAuth(event) {
  event.preventDefault();
  const email = document.getElementById('email').value;
  const password = document.getElementById('password').value;

  // Simulate API call
  setTimeout(() => {
    currentUser = { email };
    currentView = 'products';
    showProducts();
  }, 500);
}

function handleForgotPassword() {
  const email = document.getElementById('email').value;
  if (!email) {
    alert('Please enter your email address');
    return;
  }
  alert('Password reset instructions would be sent to your email');
}

function switchTab(tab) {
  currentTab = tab;
  showAuth();
}

function showAddProductModal() {
  const modal = document.getElementById('productModal');
  modal.classList.remove('hidden');
}

function closeProductModal() {
  const modal = document.getElementById('productModal');
  modal.classList.add('hidden');
  document.getElementById('productForm').reset();
}

function handleProductSubmit(event) {
  event.preventDefault();
  const name = document.getElementById('productName').value;
  const description = document.getElementById('productDescription').value;
  const price = parseFloat(document.getElementById('productPrice').value);

  const newProduct = {
    id: products.length + 1,
    name,
    description,
    price
  };

  products.push(newProduct);
  closeProductModal();
  showProducts();
}

function editProduct(id) {
  alert(`Edit product ${id} modal would show here`);
}

function deleteProduct(id) {
  alert(`Delete product ${id} confirmation would show here`);
}

// Expose functions to window
window.handleAuth = handleAuth;
window.handleForgotPassword = handleForgotPassword;
window.switchTab = switchTab;
window.showAddProductModal = showAddProductModal;
window.closeProductModal = closeProductModal;
window.handleProductSubmit = handleProductSubmit;
window.editProduct = editProduct;
window.deleteProduct = deleteProduct;

// Initialize the app
showAuth();