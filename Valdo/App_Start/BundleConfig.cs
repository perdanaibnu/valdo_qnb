using System.Web;
using System.Web.Optimization;

namespace Valdo
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                      "~/Scripts/jquery.min.js",
                      "~/Scripts/jquery-ui.min.js",
                      "~/Content/bootstrap/dist/js/bootstrap.min.js",
                      "~/Scripts/jquery.dataTables.min.js",
                      "~/Scripts/dataTables.bootstrap.min.js",
                      "~/Scripts/dataTables.checkboxes.min.js",
                      "~/Scripts/dataTables.scroller.min.js",
                      "~/Content/select2/dist/js/select2.full.min.js",
                      "~/Scripts/jquery.inputmask.bundle.min.js",
                      "~/Scripts/jquery.sparkline.min.js",
                      "~/Scripts/jquery.knob.min.js",
                      "~/Scripts/moment.min.js",
                      "~/Scripts/daterangepicker.js",
                      "~/Scripts/bootstrap3-wysihtml5.all.min.js",
                      "~/Scripts/jquery.slimscroll.min.js",
                      "~/Scripts/icheck.min.js",
                      "~/Scripts/fastclick.js",
                      "~/Content/dist/js/adminlte.min.js",
                      "~/Scripts/bootstrap-datepicker.min.js",
                      "~/Scripts/bootstrap-colorpicker.min.js",
                      "~/Scripts/bootstrap-timepicker.min.js",
                      "~/Scripts/sweetalert2.min.js",
                      "~/Scripts/jquery.blockUI.js",
                      "~/Scripts/Chart.min.js",
                      "~/Scripts/timeout-dialog.js",
                      "~/Scripts/jszip.min.js",
                      "~/Scripts/pdfmake.min.js",
                      "~/Scripts/vfs_fonts.js",
                      "~/Scripts/buttons.bootstrap4.min.js",
                      "~/Scripts/dataTables.buttons.min.js",
                      "~/Scripts/buttons.html5.min.js",
                      "~/Scripts/buttons.print.min.js",
                      "~/Scripts/buttons.colVis.min.js",
                      "~/Scripts/dataTables.searchBuilder.min.js",
                      "~/Scripts/dataTables.dateTime.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap/dist/css/bootstrap.min.css",
                      "~/Content/font-awesome/css/font-awesome.min.css",
                      "~/Content/ionicons.min.css",
                      "~/Content/dataTables.bootstrap.min.css",
                      "~/Content/dataTables.checkboxes.css",
                      "~/Content/scroller.bootstrap.min.css",
                      "~/Content/scroller.dataTables.min.css",
                      "~/Content/dist/css/AdminLTE.min.css",
                      "~/Content/_all-skins.css",
                      "~/Content/daterangepicker.css",
                      "~/Content/bootstrap3-wysihtml5.min.css",
                      "~/Content/select2/dist/css/select2.min.css",
                      "~/Content/select2/dist/css/select2-bootstrap.min.css",
                      "~/Content/bootstrap-datepicker.min.css",
                      "~/Content/timepicker.less",
                      "~/Content/iCheck/icheck-all.css",
                      "~/Content/bootstrap-colorpicker.min.css",
                      "~/Content/dist/css/style.css",
                      "~/Content/sweetalert2.min.css",
                      "~/Content/Chart.min.css",
                      "~/Content/inputmask.css",
                      "~/Content/timeout-dialog.css",
                      "~/Content/AddStyle.css",
                      "~/Content/site.css",
                      "~/Content/buttons.bootstrap4.min.css",
                      "~/Content/buttons.dataTables.min.css",
                      "~/Content/searchBuilder.dataTables.min.css",
                      "~/Content/dataTables.dateTime.min.css"));
        }
    }
}
