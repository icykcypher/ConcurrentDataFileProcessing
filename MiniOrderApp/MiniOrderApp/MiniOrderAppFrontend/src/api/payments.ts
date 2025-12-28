import { api } from './client';

export interface PaymentCreateDto {
    orderId: number;
    paidAmount: number;
}

export const createPayment = async (dto: PaymentCreateDto) => {
    const { data } = await api.post('/payments', dto);
    return data.data;
};
