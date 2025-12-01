import React from "react";
import { Button, TextField, Box, Table, TableHead, TableRow, TableCell, TableBody } from "@mui/material";
import axios from "axios";
import CommissionResultGrid from "../components/CommissionResultGrid";
import { set } from "zod";

export default function CommissionPage() {
  const [vendas, setVendas] = React.useState<
    Array<{ vendedor: string; valor: number; comissao: number | null }>
  >([]);
  const [vendedor, setVendedor] = React.useState("");
  const [valor, setValor] = React.useState<number | "">("");
  const [comissao, setComissao] = React.useState<number | null>(null);
  const [result, setResult] = React.useState<any>(null);

  function addVenda() {
    if (!vendedor || valor === "" || Number(valor) <= 0) return;
    setVendas([...vendas, { vendedor, valor: Number(valor), comissao }]);
    setVendedor("");
    setValor("");
  }

  function clearVendas() {
    setVendas([]);
    setResult(null);
  }

  async function send() {
    if (vendas.length === 0) return;
    const res = await axios.post("http://localhost:5000/api/v1/comissao", { vendas });
    setResult(res.data);
  }

  return (
    <Box sx={{ maxWidth: 800, mx: "auto", mt: 3 }}>

      {/* COMPACT FORM */}
      <Box sx={{ mb: 2 }}>
        
        {/* Row 1: Inputs */}
        <Box
          sx={{
            display: "flex",
            gap: 2,
            flexWrap: "wrap",
            mb: 1
          }}
        >
          <TextField
            label="Vendedor"
            size="small"
            value={vendedor}
            onChange={(e) => setVendedor(e.target.value)}
          />

          <TextField
            label="Valor R$"
            size="small"
            type="number"
            value={valor as any}
            onChange={(e) =>
              setValor(e.target.value ? Number(e.target.value) : "")
            }
          />

          <Button
            onClick={addVenda}
            variant="contained"
            color="success"
            size="small"
            sx={{ whiteSpace: "nowrap" }}
          >
            Adicionar
          </Button>
        </Box>

        {/* Row 2: Actions */}
        <Box sx={{ display: "flex", gap: 2 }}>
          <Button
            onClick={clearVendas}
            color="error"
            size="small"
            variant="contained"
          >
            Limpar Vendas
          </Button>

          <Button
            onClick={send}
            size="small"
            variant="contained"
            disabled={vendas.length === 0}
          >
            Enviar
          </Button>
        </Box>
      </Box>

      {/* RESULTS SECTION */}
      {result ? (
        <CommissionResultGrid result={result} />
      ) : vendas.length > 0 ? (
        <Box sx={{ overflowX: "auto", mt: 2 }}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell><strong>Vendedor</strong></TableCell>
                <TableCell><strong>Valor</strong></TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {vendas.map((v, i) => (
                <TableRow key={i}>
                  <TableCell>{v.vendedor}</TableCell>
                  <TableCell>{v.valor}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </Box>
      ) : null}
    </Box>
  );
}

