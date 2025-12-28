import { useEffect, useState } from 'react';
import { getCustomers } from '../api/customers';
import type { Customer } from "../api/customers"
import { Table } from '../components/Table';
import { Card } from '../components/Card';
import { ImportCSV } from '../components/ImportCustomerCSV';

export const Customers = () => {
  const [customers, setCustomers] = useState<Customer[]>([]);

  useEffect(() => {
    getCustomers().then(setCustomers);
  }, []);

  return (
    <Card>
      <ImportCSV />
      <h2>Customers</h2>
      <Table
        data={customers}
        columns={[
          { key: 'id', label: 'ID' },
          { key: 'name', label: 'Name' },
          { key: 'email', label: 'Email' },
          { key: 'isActive', label: 'Active' },
        ]}
      />
    </Card>
  );
};
