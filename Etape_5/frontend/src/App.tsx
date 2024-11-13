import { FunctionComponent, useEffect } from 'react'
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'

import 'bootstrap/dist/css/bootstrap.min.css';
import './App.css';


import NavbarMain from './components/navbar/NavbarComponent'

import Home from './views/HomeView'
import Register from './views/RegisterView'
import Login from './views/LoginView'
import Profile from './views/ProfileView'
import Users from './views/UsersView'


const App: FunctionComponent = () => {


  const deleteToken = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  };

  useEffect(() => {
    return () => {
      deleteToken();
    };
  }, []);

  return (
    <Router>
      <div className='App'>
        <NavbarMain />
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/register" element={<Register />} />
          <Route path="/login" element={<Login />} />
          <Route path="/profile" element={<Profile />} />
          <Route path="/users" element={<Users />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
