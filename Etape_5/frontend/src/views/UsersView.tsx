import { FunctionComponent, useState, useEffect } from 'react'
import UserList from '../components/users/list/UserListComponent'

import { User } from '../types'
import { getUsers } from '../services/userService';

const Users: FunctionComponent = () => {
    const [allUsers, setAllUsers] = useState<User[]>([]);

    const loadData = () => {
        const userStr = localStorage.getItem('user');
        const userId = userStr ? JSON.parse(userStr).id : null;

        getUsers()
            .then(data => setAllUsers(data))
            .catch(error => console.error('Erreur lors de la récupération des adresses:', error));
    };

    useEffect(() => {
        loadData();
    }, []);

    const handleDeleteUser = (userId: number) => {
        setAllUsers(prevUsers => prevUsers.filter(user => user.id !== userId));
    };

    return (
        <main className="container">
            <UserList users={allUsers} onDeleteUser={handleDeleteUser} />
        </main>
    )
}

export default Users
