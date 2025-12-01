-- Stock Movement Database Schema (SQL Server)
-- This script defines the tables for the stock movement system

-- PRODUTO table
CREATE TABLE Produto (
    Codigo BIGINT PRIMARY KEY,
    Descricao NVARCHAR(255) NOT NULL,
    Estoque INT NOT NULL DEFAULT(0),
    RowVersion ROWVERSION NOT NULL
);

-- MOVIMENTACAO_ESTOQUE table
CREATE TABLE MovimentacaoEstoque (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    PublicId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID() UNIQUE,
    CodigoProduto BIGINT NOT NULL,
    Tipo NVARCHAR(50) NOT NULL,
    Quantidade INT NOT NULL,
    Descricao NVARCHAR(1000) NULL,
    DataHora DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    SaldoAntes INT NOT NULL,
    SaldoDepois INT NOT NULL,
    CONSTRAINT FK_Movimentacao_Produto FOREIGN KEY (CodigoProduto) REFERENCES Produto(Codigo) ON DELETE NO ACTION
);

-- Index for efficient last-10 query
CREATE INDEX IX_Movimentacao_Codigo_DataHoraDesc
ON MovimentacaoEstoque (CodigoProduto, DataHora DESC);

-- Initial data seed
INSERT INTO Produto (Codigo, Descricao, Estoque)
VALUES 
    (101, 'Caneta Azul', 150),
    (102, 'Caderno Universitário', 75),
    (103, 'Borracha Branca', 200),
    (104, 'Lápis Preto HB', 320),
    (105, 'Marcador de Texto Amarelo', 90);
