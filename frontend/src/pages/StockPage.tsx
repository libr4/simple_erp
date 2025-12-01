import React from "react";
import axios from "axios";
import {
  Box,
  Button,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  TextField,
  FormHelperText,
  Typography,
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
} from "@mui/material";

type TipoMov = "ENTRADA" | "SAIDA" | "INVENTARIO";

type StockItem = {
  codigoProduto: number;
  descricaoProduto: string;
  estoque: number;
};

type Movimentacao = {
  publicId: string;
  tipo: TipoMov;
  quantidade: number;
  descricao: string;
  dataHora: string;
  saldoAntes: number;
  saldoDepois: number;
};

type ApiResponse = {
  produto: { codigo: number; descricao: string; estoque: number };
  movimentacoesRecentes: Movimentacao[];
};

export default function StockPage() {
  // form state
  const [codigoProduto, setCodigoProduto] = React.useState<number | "">("");
  const [tipo, setTipo] = React.useState<TipoMov>("ENTRADA");
  const [quantidade, setQuantidade] = React.useState<number | "">("");
  const [descricaoMov, setDescricaoMov] = React.useState("");

  // validation & server errors
  const [errors, setErrors] = React.useState<Record<string, string[]>>({});
  const [serverErrors, setServerErrors] = React.useState<Record<string, string[]>>({});
  const [apiResponse, setApiResponse] = React.useState<ApiResponse | null>(null);
  const [loading, setLoading] = React.useState(false);

  // just to show user-friendly product text in dropdown
  const stock: StockItem[] = [
    { codigoProduto: 101, descricaoProduto: "Caneta Azul", estoque: 150 },
    { codigoProduto: 102, descricaoProduto: "Caderno Universitário", estoque: 75 },
    { codigoProduto: 103, descricaoProduto: "Borracha Branca", estoque: 200 },
    { codigoProduto: 104, descricaoProduto: "Lápis Preto HB", estoque: 320 },
    { codigoProduto: 105, descricaoProduto: "Marcador de Texto Amarelo", estoque: 90 },
  ];

  // ---------- Validation ----------
  function validate() {
    const e: Record<string, string[]> = {};

    if (!codigoProduto) {
      e.codigoProduto = ["Código do produto é obrigatório."];
    }

    if (quantidade === "" || quantidade === null || Number.isNaN(Number(quantidade))) {
      e.quantidade = ["Quantidade deve ser um número inteiro."];
    } else if (!Number.isInteger(Number(quantidade))) {
      e.quantidade = ["Quantidade deve ser um inteiro."];
    } else if (Number(quantidade) <= 0) {
      e.quantidade = ["Quantidade deve ser maior que 0."];
    }

    if (!descricaoMov.trim()) {
      e.descricao = ["Descrição não pode ser vazia."];
    }

    if (!["ENTRADA", "SAIDA", "INVENTARIO"].includes(tipo)) {
      e.tipo = ["Tipo inválido."];
    }

    setErrors(e);
    return Object.keys(e).length === 0;
  }

  // ---------- Send ----------
  async function send() {
    setServerErrors({});
    // don’t clear apiResponse; we keep last good response until next success
    if (!validate()) return;

    const payload = {
      codigoProduto: Number(codigoProduto),
      tipo,
      quantidade: Number(quantidade),
      descricao: descricaoMov,
    };

    try {
      setLoading(true);
      const res = await axios.post<ApiResponse>(
        "http://localhost:5000/api/v1/estoque",
        payload
      );
      setApiResponse(res.data);
      setServerErrors({});
    } catch (err: any) {
      const data = err?.response?.data;

      if (data && !data.errors && typeof data === "object") {
        // validation object like:
        // { "Tipo": ["..."], "Quantidade": ["..."] }
        setServerErrors(data);
      } else if (data?.errors && typeof data.errors === "object") {
        // model-binding / parsing errors
        const filtered: Record<string, string[]> = {};
        Object.entries(data.errors).forEach(([k, v]) => {
          if (k !== "request") filtered[k] = v as string[];
        });
        setServerErrors(filtered);
      } else {
        setServerErrors({ geral: ["Erro inesperado."] });
      }
    } finally {
      setLoading(false);
    }
  }

  // helper for local description (for dropdown label only)
  const selectedLocalProduct = stock.find((s) => s.codigoProduto === codigoProduto) ?? null;

  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
      {/* FORM */}
      <Box
  sx={{
    display: "grid",
    gridTemplateColumns: "260px 140px 120px auto",
    columnGap: 1,
    rowGap: 1,
    alignItems: "center",
  }}
>
  {/* Row 1 */}
  <FormControl size="small">
    <InputLabel id="produto-label">Produto</InputLabel>
    <Select
      labelId="produto-label"
      label="Produto"
      value={codigoProduto}
      onChange={(e) => setCodigoProduto(e.target.value as number)}
    >
      <MenuItem value="">
        <em>Selecione</em>
      </MenuItem>
      {stock.map((p) => (
        <MenuItem key={p.codigoProduto} value={p.codigoProduto}>
          {`${p.codigoProduto} — ${p.descricaoProduto}`}
        </MenuItem>
      ))}
    </Select>
    {errors.codigoProduto && (
      <FormHelperText error>{errors.codigoProduto[0]}</FormHelperText>
    )}
    {serverErrors.codigoProduto && (
      <FormHelperText error>{serverErrors.codigoProduto[0]}</FormHelperText>
    )}
  </FormControl>

  <FormControl size="small">
    <InputLabel id="tipo-label">Tipo</InputLabel>
    <Select
      labelId="tipo-label"
      label="Tipo"
      value={tipo}
      onChange={(e) => setTipo(e.target.value as TipoMov)}
    >
      <MenuItem value="ENTRADA">ENTRADA</MenuItem>
      <MenuItem value="SAIDA">SAIDA</MenuItem>
      <MenuItem value="INVENTARIO">INVENTARIO</MenuItem>
    </Select>
    {errors.tipo && <FormHelperText error>{errors.tipo[0]}</FormHelperText>}
    {serverErrors.Tipo && (
      <FormHelperText error>{serverErrors.Tipo[0]}</FormHelperText>
    )}
  </FormControl>

  <TextField
    label="Qtdade"
    size="small"
    type="number"
    value={quantidade}
    onChange={(e) => {
      const raw = e.target.value;
      setQuantidade(raw === "" ? "" : Number(raw));
    }}
    error={Boolean(errors.quantidade || serverErrors.Quantidade || serverErrors.quantidade)}
    helperText={
      errors.quantidade?.[0] ??
      serverErrors.Quantidade?.[0] ??
      serverErrors.quantidade?.[0] ??
      ""
    }
  />

  {/* Row 2 */}
  <TextField
    label="Descrição"
    size="small"
    value={descricaoMov}
    onChange={(e) => setDescricaoMov(e.target.value)}
    sx={{ gridColumn: "1 / span 3" }}   // <-- spans all 3 upper inputs
    error={Boolean(errors.descricao || serverErrors.Descricao || serverErrors.descricao)}
    helperText={
      errors.descricao?.[0] ??
      serverErrors.Descricao?.[0] ??
      serverErrors.descricao?.[0] ??
      ""
    }
  />

  <Button
    variant="contained"
    onClick={send}
    disabled={loading}
    sx={{ height: 40 }}
  >
    {loading ? "Enviando..." : "Enviar"}
  </Button>
</Box>


        {/* tiny helper for user, not dev-y, optional */}
        {selectedLocalProduct && (
          <Typography variant="caption">
            Produto selecionado: {selectedLocalProduct.descricaoProduto}
          </Typography>
        )}
        {serverErrors.geral && (
          <Typography variant="body2" color="error">
            {serverErrors.geral[0]}
          </Typography>
        )}

      {/* RESPONSE (only if exists) */}
      {apiResponse && (
        <Box sx={{ mt: 2, maxWidth: 900 }}>
          {/* produto summary */}
          <Box sx={{ mb: 1 }}>
            <Typography variant="body1" sx={{ fontWeight: "bold" }}>
              {apiResponse.produto.codigo} — {apiResponse.produto.descricao}
            </Typography>
            <Typography variant="body2">
              Estoque atual: {apiResponse.produto.estoque}
            </Typography>
          </Box>

          {/* movimentações table */}
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Tipo</TableCell>
                <TableCell>Quantidade</TableCell>
                <TableCell>Descrição</TableCell>
                <TableCell>Data/Hora</TableCell>
                <TableCell>Saldo Antes</TableCell>
                <TableCell>Saldo Depois</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {apiResponse.movimentacoesRecentes.map((m) => (
                <TableRow key={m.publicId}>
                  <TableCell>{m.tipo}</TableCell>
                  <TableCell>{m.quantidade}</TableCell>
                  <TableCell>{m.descricao}</TableCell>
                  <TableCell>{new Date(m.dataHora).toLocaleString()}</TableCell>
                  <TableCell>{m.saldoAntes}</TableCell>
                  <TableCell>{m.saldoDepois}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </Box>
      )}
    </Box>
  );
}
