import React, { FunctionComponent, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { registerUser } from '../../services/authService';
import { RegisterFormData } from "../../types";


const FormField: FunctionComponent<{
    label: string;
    id: string;
    name: string;
    type: string;
    value: any;
    onChange: React.ChangeEventHandler<HTMLInputElement | HTMLSelectElement>;
    autoComplete?: string;
    options?: { value: string | number; text: string }[];
}> = ({ label, id, name, type, value, onChange, autoComplete, options }) => (
    <>
        <div className="form-group">
            <label htmlFor={id}>
                {label}
            </label>
            {type === 'select' ? (
                <select id={id} name={name} value={value} onChange={onChange} required className="form-control">
                    <option disabled value="">
                        ---------
                    </option>
                    {options?.map((option) => (
                        <option value={option.value} key={option.value}>
                            {option.text}
                        </option>
                    ))}
                </select>
            ) : (
                <input
                    id={id}
                    name={name}
                    type={type}
                    value={value}
                    onChange={onChange}
                    required
                    autoComplete={autoComplete}
                    className="form-control"
                />
            )}
        </div>
    </>

);

const RegisterForm: FunctionComponent = () => {
    const [data, setData] = useState<RegisterFormData>({
        role: '0',
        username: '',
        password: '',
    });
    const [errorMessage, setError] = useState<string>('');
    const navigate = useNavigate();

    const handleInputChange = (event: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = event.target;
        setData((prevData) => ({ ...prevData, [name]: value }));
    };

    const submitForm = async (event: React.FormEvent) => {
        event.preventDefault();
        const { role, username, password } = data;
        const isSuccess = await registerUser({ role, username, password }, setError);

        if (isSuccess) {
            navigate('/login');
        } else {
            setError("Échec de l'inscription.");
        }
    };

    return (
        <form onSubmit={submitForm}>
            {/* <FormField
                label="Role:"
                id="id_role"
                name="role"
                type="select"
                value={data.role}
                onChange={handleInputChange}
                options={[
                    { value: '0', text: 'User' },
                    { value: '1', text: 'Admin' },
                ]}
            /> */}
            <FormField
                label="Email:"
                id="id_email"
                name="username"
                type="email"
                value={data.username}
                onChange={handleInputChange}
                autoComplete="email"
            />
            <FormField
                label="Password:"
                id="id_password"
                name="password"
                type="password"
                value={data.password}
                onChange={handleInputChange}
                autoComplete="new-password"
            />
            <button className="btn btn-primary" type="submit">
                Register
            </button>
        </form>
    );
};

export default RegisterForm;
