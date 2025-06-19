import axios, { type AxiosResponse } from 'axios'

const axiosApi = axios.create({
  baseURL: '/api/v1',
  timeout: 10000,
  headers: { 'content-type': 'application/json' }
})

export default {
  deleteAccount(email: string, password: string): Promise<AxiosResponse<void>> {
    return axiosApi.post('auth/user/delete-account', { email, password })
  }
}
