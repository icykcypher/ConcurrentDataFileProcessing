import { useState } from 'react';
import { importCustomers } from '../api/customers';

export const ImportCSV = () => {
  const [file, setFile] = useState<File | null>(null);
  const [message, setMessage] = useState('');

  const handleSubmit = async () => {
    if (!file) return setMessage('Select a file first');
    try {
      await importCustomers(file);
      setMessage('Import successful');
    } catch (err: unknown) {
      setMessage('Error: ' + err);
    }
  };

  return (
    <div style={{ marginBottom: 16 }}>
      <input type="file" accept=".csv" onChange={(e) => setFile(e.target.files?.[0] || null)} />
      <button onClick={handleSubmit} style={{ marginLeft: 8 }}>Upload</button>
      {message && <p>{message}</p>}
    </div>
  );
};
