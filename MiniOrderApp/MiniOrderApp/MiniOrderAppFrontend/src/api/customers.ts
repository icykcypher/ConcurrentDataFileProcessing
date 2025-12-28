import { api } from './client';

export interface Customer {
  id: number;
  name: string;
  surname: string;
  email: string;
  isActive: boolean;
}

export const getCustomers = async (): Promise<Customer[]> => {
  const { data } = await api.get('/customers');
  return data.data;
};

export const createCustomer = async (data: {
  name: string;
  surname: string;
  email: string;
  isActive: boolean;
}): Promise<Customer> => {
  const { data: customer } = await api.post('/customers', data);
  return customer;
};

export const importCustomers = async (file: File) => {
  const formData = new FormData();
  formData.append('file', file);

 return api.post('/import/customers', formData, 
  { headers: { 'Content-Type': 'multipart/form-data' }, });
};
