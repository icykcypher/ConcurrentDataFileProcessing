import type { Product } from '../api/products';
import { useCart } from '../context/CartContext';

export const ProductCard = ({ product }: { product: Product }) => {
  const { addToCart } = useCart();

  return (
    <div style={{ border: '1px solid #ccc', padding: 8, margin: 8 }}>
      <h3>{product.name}</h3>
      <p>${product.price.toFixed(2)}</p>
      <button onClick={() => addToCart(product)}>Buy</button>
    </div>
  );
};
