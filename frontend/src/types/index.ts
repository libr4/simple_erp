export interface Venda {
  vendedor: string
  valor: number
}

export interface VendasPayload {
  vendas: Venda[]
}

export interface ItemEstoque {
  codigoProduto: number
  descricaoProduto: string
  estoque: number
}

export interface EstoquePayload {
  estoque: ItemEstoque[]
}

export interface ItemComissao {
  valor: number;
  comissao: number;
}

export interface ComissaoVendedor {
  vendedor: string;
  totalVendas: number;
  comissaoTotal: number;
  itens: ItemComissao[];
}

export type ComissaoResponse = ComissaoVendedor[];