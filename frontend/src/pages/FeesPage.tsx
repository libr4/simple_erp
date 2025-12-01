import React from "react";
import { Box, TextField, Button, Typography, Paper } from "@mui/material";
import axios from "axios";
import { useSearchParams } from "react-router-dom";

interface FeeQueryParams {
  vencimento?: string;
  valor?: number;
}

export default function FeesPage() {
  const [vencimento, setVencimento] = React.useState("");
  const [valor, setValor] = React.useState<number | "">("");
  const [result, setResult] = React.useState<any>(null);

  const [searchParams, setSearchParams] = useSearchParams();

  // Load params on first mount
  React.useEffect(() => {
    const venc = searchParams.get("vencimento") ?? "";
    const valStr = searchParams.get("valor") ?? "";

    setVencimento(venc);
    setValor(valStr ? Number(valStr) : "");
  }, []);

  function syncURL(venc: string, val: string | number | "") {
    const params: any = {};
    if (venc) params.vencimento = venc;
    if (val !== "" && !isNaN(Number(val))) params.valor = String(val);
    setSearchParams(params);
  }

  async function send() {
    if (!vencimento || valor === "") return;
    const res = await axios.get("http://localhost:5000/api/v1/fees", {
      params: { vencimento, valor },
    });
    setResult(res.data);
  }

  function onValueChange(e: React.ChangeEvent<HTMLInputElement>) {
    const val = e.target.value;
    const parsed = val ? Number(val) : "";
    setValor(parsed);
    syncURL(vencimento, parsed);
  }

  function onVencimentoChange(e: React.ChangeEvent<HTMLInputElement>) {
    const val = e.target.value;
    setVencimento(val);
    syncURL(val, valor);
  }

  return (
    <Box sx={{ maxWidth: 600, mx: "auto", mt: 3 }}>
      {/* COMPACT FORM */}
      <Box sx={{ mb: 2 }}>
        <Box
          sx={{
            display: "flex",
            gap: 2,
            flexWrap: "wrap",
            alignItems: "center",
            mb: 1,
          }}
        >
          <TextField
            label="Vencimento (DD-MM-YYYY)"
            size="small"
            value={vencimento}
            onChange={onVencimentoChange}
          />

          <TextField
            label="Valor R$"
            size="small"
            type="number"
            value={valor as any}
            onChange={onValueChange}
          />

          <Button
            variant="contained"
            size="small"
            onClick={send}
            disabled={!vencimento || valor === ""}
            sx={{ whiteSpace: "nowrap" }}
          >
            Enviar
          </Button>
        </Box>
      </Box>

      {/* RESULT DISPLAY */}
      {result && (
        <Paper
          sx={{
            p: 2,
            mt: 2,
            background: "#fafafa",
          }}
        >
          <Typography variant="subtitle1" sx={{ mb: 1, fontWeight: 600 }}>
            Resultado
          </Typography>

          <Typography><strong>Valor original:</strong> {result.valorOriginal}</Typography>
          <Typography><strong>Dias de atraso:</strong> {result.diasAtraso}</Typography>
          <Typography><strong>Juros:</strong> {result.juros}</Typography>
          <Typography><strong>Valor com juros:</strong> {result.valorComJuros}</Typography>
        </Paper>
      )}
    </Box>
  );
}
