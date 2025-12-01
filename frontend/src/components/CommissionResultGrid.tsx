import { Table, TableHead, TableRow, TableCell, TableBody, Typography, Box } from '@mui/material'
import React from 'react'
import { ComissaoResponse } from '../types'

export default function CommissionResultGrid({ result }: { result: ComissaoResponse }) {
  return (
    <>
      {result.map((v, i) => (
        <Table key={i} sx={{ mb: 1 }}>
          <TableHead>
            <TableRow>
              <TableCell colSpan={2}>
                <Typography fontWeight={1000} variant="h5" sx={{ mt: 1, mb: 1 }}>
                  Resumo do Vendedor: {v.vendedor}
                </Typography>
              </TableCell>
            </TableRow>
            <TableRow>
              <TableCell>Total de Vendas</TableCell>
              <TableCell>Comissão Total</TableCell>
            </TableRow>
          </TableHead>

          <TableBody>
            <TableRow>
              <TableCell>{v.totalVendas}</TableCell>
              <TableCell>{v.comissaoTotal}</TableCell>
            </TableRow>

            <TableRow>
              <TableCell colSpan={2}>
                <Typography fontWeight={1000} sx={{ pt: 1, mb: 1 }}>
                  Comissões individuais de {v.vendedor}
                </Typography>
              </TableCell>
            </TableRow>

            <TableRow>
              <TableCell>Valor</TableCell>
              <TableCell>Comissão</TableCell>
            </TableRow>

            {v.itens?.map((item, j) => (
              <TableRow key={j}>
                <TableCell>{item.valor}</TableCell>
                <TableCell>{item.comissao}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      ))}
    </>
  );
}

