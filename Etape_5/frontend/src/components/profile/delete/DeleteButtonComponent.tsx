import { FunctionComponent } from 'react'
import { useNavigate } from 'react-router-dom';

import { getCurrrentUser, deleteUserAccount } from '../../../services/userService';

import { CurrentUser } from '../../../types';


const DeleteUserButton: FunctionComponent = () => {
    const navigate = useNavigate();

    const handleDelete = async () => {
        const isConfirmed = window.confirm('Êtes-vous sûr de vouloir supprimer votre compte ?');

        if (isConfirmed) {
            const currentUserData: CurrentUser = await getCurrrentUser();
            const userId: number = currentUserData.id;
            await deleteUserAccount(userId);
            navigate('/');
        }
    };

    return (
        <button className="btn btn-danger" onClick={handleDelete}>
            Delete account
        </button>
    );
};

export default DeleteUserButton;
