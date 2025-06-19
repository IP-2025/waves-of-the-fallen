import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import SupportView from '../views/SupportView.vue'
import TermsView from '../views/TermsView.vue'
import PrivacyView from '../views/PrivacyView.vue'
import NotFoundView from '../views/NotFoundView.vue'
import DeleteAccountView from '../views/DeleteAccountView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: HomeView,
    },
    {
      path: '/support',
      component: SupportView,
    },
    {
      path: '/terms',
      component: TermsView
    },
    {
      path: '/privacy',
      component: PrivacyView
    },
    {
      path: '/delete-account',
      component: DeleteAccountView
    },
    { path: '/:pathMatch(.*)*', component: NotFoundView } // Catch-all for 404
  ],
})

export default router
