import { CustomError } from "./customErorr";

export class InternalServerError extends CustomError {
  constructor(message: string) {
    super(message, 500);
  }
}
