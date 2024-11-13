import React, { useState, useEffect, FunctionComponent, ChangeEvent, FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { authenticateUser, getUser } from '../../services/authService';


const InputField: FunctionComponent<{
    label: string,
    type: string,
    id: string,
    name: string,
    value: string,
    onChange: (e: ChangeEvent<HTMLInputElement>) => void,
    autoComplete?: string
}> = ({ label, type, id, name, value, onChange, autoComplete }) => (
    <div className="form-group">
        <label htmlFor={id}>{label}</label>
        <input
            type={type}
            id={id}
            name={name}
            value={value}
            onChange={onChange}
            required
            autoComplete={autoComplete}
            className="form-control"
        />
    </div>
);


const LoginForm: FunctionComponent = () => {
    const [formData, setFormData] = useState<{ username: string, password: string }>({ username: '', password: '' });
    const [errorMessage, setError] = useState<string>('');
    const navigate = useNavigate();

    useEffect(() => {
        setError('');
    }, [formData.username, formData.password]);

    const handleInputChange = (e: ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setFormData(prevData => ({ ...prevData, [name]: value }));
    };

    const submitForm = async (e: FormEvent) => {
        e.preventDefault();

        const getToken = await authenticateUser(formData.username, formData.password, setError);
        if (getToken) {
            const getUserData = await getUser(setError);
            if (getUserData) {
                navigate('/profile');
            }
        }
    };

    return (
        <form onSubmit={submitForm}>
            <InputField label="Email:" type="email" id="id_email" name="username" value={formData.username} onChange={handleInputChange} autoComplete="email" />
            <InputField label="Password:" type="password" id="id_password" name="password" value={formData.password} onChange={handleInputChange} />
            <button className="btn btn-primary" type="submit">Log in</button>
            {errorMessage && <p style={{ color: 'red' }}>{errorMessage}</p>}
        </form>
    );
};

export default LoginForm;
