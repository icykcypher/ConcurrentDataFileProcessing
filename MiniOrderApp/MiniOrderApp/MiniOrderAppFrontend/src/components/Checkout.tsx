import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useCart } from '../context/CartContext';
import { createCustomer, getCustomers } from '../api/customers';
import { createOrder } from '../api/orders';
import { createPayment } from '../api/payments';

export const Checkout = () => {
  const { items, clearCart } = useCart();
  const navigate = useNavigate();

  const [name, setName] = useState('');
  const [surname, setSurname] = useState('');
  const [email, setEmail] = useState('');
  const [loading, setLoading] = useState(false);
  const [message] = useState('');

  const total = items.reduce(
    (sum: number, item: { price: number; quantity: number }) => sum + item.price * item.quantity,
    0
  );

  const handlePayment = async (e: React.FormEvent) => {
    e.preventDefault();
    if (items.length === 0) return alert("Cart is empty");
    setLoading(true);

    const total = items.reduce((sum, item) => sum + item.price * item.quantity, 0);

    try {
        let customerId = localStorage.getItem("customerId");
        if (!customerId || isNaN(Number(customerId))) {
        const customers = await getCustomers();
        const existing = customers.find(c => c.email.toLowerCase() === email.toLowerCase());

        if (existing) {
          customerId = String(existing.id);
        } else {
          const newCustomer = await createCustomer({ name, surname, email, isActive: true });
          customerId = String(newCustomer.id);
        }

        localStorage.setItem('customerId', customerId);
      }
        

        // 2. Создаём заказ
        const orderItems = items.map(i => ({ productId: i.id, quantity: i.quantity }));
        const order = (await createOrder(Number(customerId), orderItems)).data;
        console.log(order)
        if (!order) {
            alert("Failed to create order. Cannot proceed with payment.");
            return;
        }

        await createPayment({
            orderId: order,
            paidAmount: total
        });



        alert(`Payment successful! Order #${order}`);
        clearCart();
        navigate("/");
    } catch (err) {
        console.error(err);
        alert("Payment failed. Check console.");
    } finally {
        setLoading(false);
    }
};
  return (
    <div style={{ marginTop: 16 }}>
      <h2>Checkout</h2>
      <input placeholder="Name" value={name} onChange={e => setName(e.target.value)} />
      <input placeholder="Surname" value={surname} onChange={e => setSurname(e.target.value)} />
      <input placeholder="Email" value={email} onChange={e => setEmail(e.target.value)} />
      <p>Total: ${total.toFixed(2)}</p>
      <button onClick={handlePayment} disabled={loading}>
        {loading ? 'Processing...' : 'Place Order & Pay'}
      </button>
      {message && <p>{message}</p>}
    </div>
  );
};
