import { FunctionComponent } from "react";
import { Link } from 'react-router-dom';

const HomeContent: FunctionComponent = () => {
    return (
        <div className="container text-center">
            <h1 className="my-4">CG Network</h1>
            <h2 className="mb-4">The network designed for digital artists and studios.</h2>
            <p className="mb-4">
                Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod
                tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim
                veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea
                commodo consequat. Duis aute irure dolor in reprehenderit in voluptate
                velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat
                cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id
                est laborum.
            </p>
            <div className="d-flex justify-content-center mb-4">
                <Link className="btn btn-primary mx-2" to="/register">Register</Link>
                <Link className="btn btn-primary mx-2" to="/login">Login</Link>
            </div>
        </div>
    );
}

export default HomeContent;
