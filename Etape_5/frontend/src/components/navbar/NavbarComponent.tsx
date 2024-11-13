import React, { FunctionComponent } from "react";
import { Link, useNavigate } from 'react-router-dom';

const NavbarMain: FunctionComponent = () => {
    const NotConnectedLinks = () => (
        <div className="d-flex justify-content-end">
            <Link className="btn btn-primary mx-1" to="/register">Register</Link>
            <Link className="btn btn-primary mx-1" to="/login">Login</Link>
        </div>
    );

    const ConnectedLinks = () => (
        <div className="d-flex justify-content-end">
            <Link className="btn btn-primary mx-1" to="/profile">Profile</Link>
            <Link className="btn btn-primary mx-1" to="/users">Users</Link>
            <Link className="btn btn-primary mx-1" to="/" onClick={logout}>Logout</Link>
        </div>
    );

    const navigate = useNavigate();
    const user = localStorage.getItem('user');

    const logout = (e: React.MouseEvent<HTMLAnchorElement, MouseEvent>) => {
        e.preventDefault();
        localStorage.removeItem('user');
        localStorage.removeItem('token');
        navigate('/');
    };

    return (
        <header className="mb-4">
            <nav className="navbar navbar-expand-lg navbar-dark bg-primary fixed-top">
                <div className="container d-flex justify-content-between">
                    <Link className="navbar-brand" to="/">HOME</Link>
                    {!user ? <NotConnectedLinks /> : <ConnectedLinks />}
                </div>
            </nav>
        </header>
    );
};

export default NavbarMain;
