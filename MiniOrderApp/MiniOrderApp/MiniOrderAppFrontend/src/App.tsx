import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import { Home } from './pages/Home';
import { Customers } from './pages/Customers';
import { Products } from './pages/Products';
import './styles/global.scss';
import { CartProvider } from './context/CartContext';

export const App = () => {
  return (
    <CartProvider>
    <Router>
      <nav style={{ display: 'flex', gap: 16, padding: 16, backgroundColor: '#eee' }}>
        <Link to="/">Home</Link>
        <Link to="/customers">Customers</Link>
        <Link to="/products">Products</Link>
      </nav>
      <div style={{ padding: 16 }}>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/customers" element={<Customers />} />
          <Route path="/products" element={<Products />} />
        </Routes>
      </div>
    </Router>
    </CartProvider>
  );
};

export default App;
