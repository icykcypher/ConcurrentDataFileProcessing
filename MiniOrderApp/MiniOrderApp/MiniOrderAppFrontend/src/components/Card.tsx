import React from 'react';
import '../styles/global.scss';

export const Card: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  return <div style={{ padding: 16, borderRadius: 8, boxShadow: '0 2px 8px rgba(0,0,0,0.1)', backgroundColor: 'white', marginBottom: 16 }}>{children}</div>;
};
