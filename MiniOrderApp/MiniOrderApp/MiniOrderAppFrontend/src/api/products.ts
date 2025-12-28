import { api } from './client';

export interface Product {
  id: number;
  name: string;
  price: number;
  category: string;
}

export const getProducts = async (): Promise<Product[]> => {
  const { data } = await api.get('/products');
  return data.data;
};

export const importProducts = async (file: File) => {
  const formData = new FormData();
  formData.append('file', file);

  return api.post('/import/products', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });
};
