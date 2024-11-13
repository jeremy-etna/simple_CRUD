import { AddressRequest, User, CurrentUser } from '../types'


const apiUrl = process.env.REACT_APP_API_URL


export const getCurrrentUser = async (): Promise<CurrentUser> => {
  const token = localStorage.getItem('token')

  if (!token) {
    throw new Error("Token undefined");
  }

  try {
    const response = await fetch(`${apiUrl}/me`, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    const currentUser = await response.json();
    return currentUser;
  } catch (error) {
    console.error('An error occurred:', error);
    return {} as User;
  }
};


export const getUsers = async (): Promise<User[]> => {
  const token = localStorage.getItem('token')

  if (!token) {
    throw new Error("Token undefined");
  }

  try {
    const response = await fetch(`${apiUrl}/user`, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    const users = await response.json();
    return users;
  } catch (error) {
    console.error('An error occurred:', error);
    return [];
  }
};



export const deleteUserAccount = async (userId: number): Promise<boolean> => {
  const token = localStorage.getItem('token')

  if (!token) {
    console.error("Token undefined");
    return false;
  }

  try {
    const response = await fetch(`${apiUrl}/user/${userId}`, {
      method: 'DELETE',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });

    if (!response.ok) {
      console.error('Failed to delete the user');
      return false;
    }

    return true;
  } catch (error) {
    console.error('An error occurred:', error);
    return false;
  }
};

