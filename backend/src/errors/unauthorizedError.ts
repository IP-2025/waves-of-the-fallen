import { CustomError } from "./customErorr";

export class UnauthorizedError extends CustomError {
  constructor(message: string) {
    super(message, 401);
  }
}
