import React from "react";
import { Container, Tabs, Tab, Box } from "@mui/material";
import CommissionPage from "./pages/CommissionPage";
import StockPage from "./pages/StockPage";
import FeesPage from "./pages/FeesPage";
import logo from "./assets/target.svg"

export default function App(){
  const [tab, setTab] = React.useState(0);

  return (
    <Container sx={{ mt: 4 }}>
      <Tabs value={tab} onChange={(_,v)=>setTab(v)} centered>
        <Tab
          value={-1}  // ← dummy tab
          icon={<img src={logo} style={{ height: 28 }} />}
          disabled          // ← not selectable
          sx={{ minWidth: 10 }} // keeps it visually tight
        />
        <Tab value={0} label="Comissão" />
        <Tab value={1} label="Estoque" />
        <Tab value={2} label="Juros" />
      </Tabs>

      <Box
  sx={{
    mt: 4,
    display: "flex",
    justifyContent: "center",
    width: "100%",
  }}
>
  <Box
    sx={{
      width: "100%",
      maxWidth: 700,

      border: "1px solid #ddd",
      borderRadius: 2,
      boxShadow: 1,

      height: "80vh",      // almost full screen
      overflow: "hidden",  // prevents the border from stretching
      display: "flex",
      flexDirection: "column",
    }}
  >
    <Box
      sx={{
        flex: 1,
        overflowY: "auto",
        overflowX: "auto",  // scroll horizontally if table is wide
        p: 3,
      }}
    >
      {tab === 0 && <CommissionPage />}
      {tab === 1 && <StockPage />}
      {tab === 2 && <FeesPage />}
    </Box>
  </Box>
</Box>

    </Container>
  );
}

