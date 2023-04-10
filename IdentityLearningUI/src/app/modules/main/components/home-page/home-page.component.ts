import { Component, OnInit, AfterViewInit, ChangeDetectorRef, ViewEncapsulation, HostListener, ViewChild, ElementRef } from '@angular/core';
import { NavbarServices } from '../../services/navBarServices/navBarService.service';
import { AuthenticationService } from 'src/app/modules/auth/services/authServices/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.scss'],
})
export class HomePageComponent {
  isInside = false;
  toogleStatus:string = "";
  selectedNavMenu: string = '';

  constructor(
    private navBarService: NavbarServices,
    private changeDetector: ChangeDetectorRef,
    private authService: AuthenticationService,
    private router: Router
  ){}
  

  ngOnInit(){
    this.UpdateSelectedNavMenuOnInit();
  }

  ToogleNavBar(){
    if(this.toogleStatus){
      this.toogleStatus = "";
    }else{
      this.toogleStatus = "minimized"
    }
  }
  
  SelectNavMenu(menuName:string){
    this.selectedNavMenu = menuName;
  }

  private UpdateSelectedNavMenuOnInit(){
    this.navBarService.selectNavBarServiceEmitter.subscribe((data: string) =>{
      this.SelectNavMenu(data);
    });
    this.changeDetector.detectChanges();
    
  }


  Logout(){
    this.authService.onSignOut();
    this.router.navigate(['SignIn']);
  }

  @HostListener('window:resize', ['$event'])
  onWindowResize() {
    if(window.innerWidth < 1049){
      this.toogleStatus = "minimized";
    }else{
      this.toogleStatus = "";
    }
  }
}
