import { HttpInterceptorFn } from '@angular/common/http';
import { environment } from '../../../environments/environment';

export const tenantInterceptor: HttpInterceptorFn = (request, next) =>
  next(request.clone({
    setHeaders: {
      'X-User-Id': String(environment.userId),
      'X-Company-Id': String(environment.companyId)
    }
  }));
