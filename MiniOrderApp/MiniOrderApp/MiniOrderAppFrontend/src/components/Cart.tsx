import { useState } from 'react';
import { useCart } from '../context/CartContext';
import { createCustomer } from '../api/customers';
import { createOrder } from '../api/orders';

export const Cart = () => {
  const { items, removeFromCart, clearCart } = useCart();
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState('');

  const [name, setName] = useState('');
  const [surname, setSurname] = useState('');
  const [email, setEmail] = useState('');

  const total = items.reduce((sum, i) => sum + i.price * i.quantity, 0);

  const handleCheckout = async () => {
    if (items.length === 0) return alert('Cart is empty');
    if (!name || !surname || !email) return alert('Fill in all customer fields');

    setLoading(true);
    setMessage('');

    try {
      let customerId = localStorage.getItem('customerId');
      if (!customerId) {
        const isActive = true;
        const customer = await createCustomer({ name, surname, email, isActive });
        customerId = String(customer.id);
        localStorage.setItem('customerId', customerId);
      }

      // Формируем заказ
      const orderItems = items.map(i => ({ productId: i.id, quantity: i.quantity }));
      const order = await createOrder(Number(customerId), orderItems);

      setMessage(`Order #${order.id} successfully created! Total: $${total.toFixed(2)}`);
      clearCart();
    } catch (err: unknown) {
      console.error(err);
      setMessage('Failed to create order: ');
    } finally {
      setLoading(false);
    }
  };

  if (items.length === 0) return <div>Cart is empty</div>;

  return (
    <div style={{ border: '1px solid #ccc', padding: 8, margin: 8 }}>
      <h2>Cart</h2>
      {items.map(i => (
        <div key={i.id}>
          {i.name} x {i.quantity} - ${i.price * i.quantity}
          <button onClick={() => removeFromCart(i.id)}>Remove</button>
        </div>
      ))}
      <p>Total: ${total.toFixed(2)}</p>

      <div style={{ marginTop: 8 }}>
        <h3>Customer info</h3>
        <input placeholder="Name" value={name} onChange={e => setName(e.target.value)} />
        <input placeholder="Surname" value={surname} onChange={e => setSurname(e.target.value)} />
        <input placeholder="Email" value={email} onChange={e => setEmail(e.target.value)} />
      </div>

      <button onClick={handleCheckout} disabled={loading} style={{ marginTop: 8 }}>
        {loading ? 'Processing...' : 'Checkout'}
      </button>
      <button onClick={clearCart} style={{ marginLeft: 8 }}>Clear Cart</button>
      {message && <p>{message}</p>}
    </div>
  );
};
