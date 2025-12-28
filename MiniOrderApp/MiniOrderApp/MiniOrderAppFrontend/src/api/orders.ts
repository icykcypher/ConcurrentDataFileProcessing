import { api } from './client';
import type { CartItem } from '../context/CartContext';

export interface Order {
  id: number;
  items: CartItem[];
}

export const createOrder = async (
  customerId: number,
  items: { productId: number; quantity: number }[]
) => {
  const { data } = await api.post('/orders', { customerId, items });
  return data;
};