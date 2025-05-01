import { CustomError } from './customErorr';

export class UnauthorizedError extends CustomError {
  constructor(message: string) {
    super(message, 401);
    Object.setPrototypeOf(this, new.target.prototype);
  }
}
