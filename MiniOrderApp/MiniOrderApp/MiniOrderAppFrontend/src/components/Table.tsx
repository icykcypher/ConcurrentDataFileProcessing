interface Column<T> {
  key: keyof T;
  label: string;
}

interface TableProps<T> {
  data: T[];
  columns: Column<T>[];
}

export const Table = <T extends Record<string, any>>({ data, columns }: TableProps<T>) => {
  return (
    <table className="w-full border-collapse">
      <thead>
        <tr>
          {columns.map((col) => (
            <th key={String(col.key)} className="border p-2 text-left">
              {col.label}
            </th>
          ))}
        </tr>
      </thead>
      <tbody>
        {data.map((row, i) => (
          <tr key={i} className="odd:bg-gray-100">
            {columns.map((col) => (
              <td key={String(col.key)} className="border p-2">
                {row[col.key] != null ? String(row[col.key]) : ''}
              </td>
            ))}
          </tr>
        ))}
      </tbody>
    </table>
  );
};
