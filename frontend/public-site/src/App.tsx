import { Route, Routes } from 'react-router-dom';
import Layout from './components/Layout';
import HomePage from './pages/HomePage';
import MobilesListPage from './pages/MobilesListPage';
import MobileDetailPage from './pages/MobileDetailPage';
import AccessoriesListPage from './pages/AccessoriesListPage';
import AccessoryDetailPage from './pages/AccessoryDetailPage';
import RepairServicesPage from './pages/RepairServicesPage';
import NotFoundPage from './pages/NotFoundPage';

function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route path="/" element={<HomePage />} />
        <Route path="/mobiles" element={<MobilesListPage />} />
        <Route path="/mobiles/:id" element={<MobileDetailPage />} />
        <Route path="/accessories" element={<AccessoriesListPage />} />
        <Route path="/accessories/:id" element={<AccessoryDetailPage />} />
        <Route path="/repairs" element={<RepairServicesPage />} />
        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  );
}

export default App;
