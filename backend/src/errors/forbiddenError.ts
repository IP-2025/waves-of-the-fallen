import { CustomError } from './customErorr';

export class ForbiddenError extends CustomError {
  constructor(message: string) {
    super(message, 403);
  }
}
