import { RegisterFormData } from "../types";

const apiUrl = process.env.REACT_APP_API_URL


export const registerUser = async (
    { role, username, password }: RegisterFormData,
    setError: React.Dispatch<React.SetStateAction<string>>
): Promise<boolean> => {
    try {
        const response = await fetch(`${apiUrl}/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ role, username, password }),
        });

        if (!response.ok) {
            const errorData = await response.json();
            const errorMessage = errorData.message || "Erreur lors de l'inscription.";
            setError(errorMessage);
            return false;
        }

        return true;
    } catch (error) {
        setError("Erreur. Essayez à nouveau plus tard.");
        return false;
    }
};



export const authenticateUser = async (
    username: string,
    password: string,
    setError: React.Dispatch<React.SetStateAction<string>>
): Promise<any> => {
    try {
        const response = await fetch(`${apiUrl}/authenticate`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, password }),
        })

        if (response.ok) {
            const token = await response.json()
            const tokenValue = token.token.replace('"', '')
            localStorage.setItem('token', tokenValue)
            return true;
        } else {
            setError("Email or password is incorrect.");
            return false;
        }
    } catch (error) {
        setError("An error occurred when getting authorization. Please try again.");
        return false;
    }
}


export const getUser = async (
    setError: React.Dispatch<React.SetStateAction<string>>
): Promise<any> => {
    const token = localStorage.getItem('token')

    if (!token) {
        throw new Error("Token undefined");
    }

    const response = await fetch(`${apiUrl}/me`, {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        },
    })
    if (response.ok) {
        const user = await response.json()
        localStorage.setItem('user', JSON.stringify(user))
        return user;
    } else {
        setError("An error occurred when getting user. Please try again.");
        return false;
    }
}
