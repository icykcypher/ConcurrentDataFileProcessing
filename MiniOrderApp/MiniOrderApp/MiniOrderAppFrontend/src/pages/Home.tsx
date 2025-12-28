import { useEffect, useState } from 'react';
import { getProducts } from '../api/products';
import type { Product } from '../api/products';
import { ProductCard } from '../components/ProductCard';
import { Checkout } from '../components/Checkout';

export const Home = () => {
  const [products, setProducts] = useState<Product[]>([]);

  useEffect(() => {
    getProducts().then(setProducts);
  }, []);

  return (
    <div>
      <h1>Mini E-Shop</h1>
      <div style={{ display: 'flex', flexWrap: 'wrap' }}>
        {products.map(p => (
          <ProductCard key={p.id} product={p} />
        ))}
      </div>
      <Checkout />
    </div>
  );
};
