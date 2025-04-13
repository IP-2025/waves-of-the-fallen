import { CustomError } from "./customErorr";

export class ConflictError extends CustomError {
  constructor(message: string) {
    super(message, 409);
  }
}
