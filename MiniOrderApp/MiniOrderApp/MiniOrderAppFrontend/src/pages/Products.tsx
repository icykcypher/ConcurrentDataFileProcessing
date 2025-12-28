import { useEffect, useState } from 'react';
import { getProducts } from '../api/products';
import type { Product } from '../api/products';
import { Table } from '../components/Table';
import { Card } from '../components/Card';
import { ImportCSV } from '../components/ImportProductCSV';

export const Products = () => {
  const [products, setProducts] = useState<Product[]>([]);

  useEffect(() => {
    getProducts().then(setProducts);
  }, []);

  return (
    <>
      <ImportCSV />
      <Card>
        <h2>Products</h2>
        <Table
          data={products}
          columns={[
            { key: 'id', label: 'ID' },
            { key: 'name', label: 'Name' },
            { key: 'price', label: 'Price' },
            { key: 'category', label: 'Category' },
          ]}
        />
      </Card>
    </>
  );
};
