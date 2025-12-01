import { Table, TableHead, TableRow, TableCell, TableBody, Typography, Box } from '@mui/material'
import React from 'react'
import { ItemEstoque } from '../types'

export default function StockResultGrid({result}:{result:ItemEstoque[]}) {
  return (
    <Table>
        <TableHead>
            <TableCell>Código</TableCell>
            <TableCell>Descrição</TableCell>
            <TableCell>Estoque (qtd)</TableCell>
        </TableHead>
            <TableBody>
                {(result ?? []).map((v,i)=>(
                    <TableRow>
                        <TableCell>{v.codigoProduto}</TableCell>
                        <TableCell>{v.descricaoProduto}</TableCell>
                        <TableCell>{v.estoque}</TableCell>
                    </TableRow>
                ))}
            </TableBody>
    </Table>
  )
}
