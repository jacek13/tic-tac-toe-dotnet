import { HTTP_INTERCEPTORS, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { AuthService } from "./auth/auth.service";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private readonly authService: AuthService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler) 
  {
    const authToken = this.authService.getAuthorizationToken();
    if (authToken)
    {
        const authReq = req.clone({
          headers: req.headers.set('Authorization', authToken)
        });
        return next.handle(authReq);
    }
    return next.handle(req);
  }
}

export const AuthInterceptorProvider = {
    provide: HTTP_INTERCEPTORS,
    useClass: AuthInterceptor,
    multi: true
}