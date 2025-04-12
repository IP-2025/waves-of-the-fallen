import { CustomError } from "./customErorr";

export class BadRequestError extends CustomError {
  constructor(message: string) {
    super(message, 400);
  }
}
