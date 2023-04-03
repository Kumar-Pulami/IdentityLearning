import { Component, OnInit, AfterViewInit, ChangeDetectorRef, ViewEncapsulation } from '@angular/core';
import { NavbarServices } from '../../services/navBarServices/navBarService.service';

@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.scss'],
})
export class HomePageComponent {
  toogleStatus:string = "";
  selectedNavMenu: string = '';
  
  constructor(
    private navBarService: NavbarServices,
    private changeDetector: ChangeDetectorRef
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
}
