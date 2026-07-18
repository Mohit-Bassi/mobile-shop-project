import { Route, Routes } from 'react-router-dom';
import AdminLayout from './components/AdminLayout';
import ProtectedRoute from './auth/ProtectedRoute';
import LoginPage from './pages/LoginPage';
import DashboardPage from './pages/DashboardPage';
import MobilesPage from './pages/MobilesPage';
import AccessoriesPage from './pages/AccessoriesPage';
import CategoriesPage from './pages/CategoriesPage';
import RepairServicesPage from './pages/RepairServicesPage';
import InquiriesPage from './pages/InquiriesPage';

function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route
        element={
          <ProtectedRoute>
            <AdminLayout />
          </ProtectedRoute>
        }
      >
        <Route path="/" element={<DashboardPage />} />
        <Route path="/mobiles" element={<MobilesPage />} />
        <Route path="/accessories" element={<AccessoriesPage />} />
        <Route path="/categories" element={<CategoriesPage />} />
        <Route path="/repair-services" element={<RepairServicesPage />} />
        <Route path="/inquiries" element={<InquiriesPage />} />
      </Route>
    </Routes>
  );
}

export default App;
