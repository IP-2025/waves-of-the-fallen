import { CustomError } from "./customErorr";

export class NotFoundError extends CustomError {
  constructor(message: string) {
    super(message, 404);
  }
}
